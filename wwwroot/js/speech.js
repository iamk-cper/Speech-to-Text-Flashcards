window.speechRecognitionInterop = {
    startListening: function (dotnetHelper, language) {
        const recognition = new (window.SpeechRecognition || window.webkitSpeechRecognition)();
        recognition.lang = language || 'pl-PL';
        recognition.continuous = false;
        recognition.interimResults = false;

        recognition.onresult = function (event) {
            const transcript = event.results[0][0].transcript;
            dotnetHelper.invokeMethodAsync('ProcessSpeech', transcript);
        };

        recognition.onerror = function (event) {
            console.error('Recognition error:', event.error);
        };

        recognition.start();
    }
};

window.speechSynthesisInterop = {
    speak: function (text, lang) {
        if (!('speechSynthesis' in window)) {
            alert("Ta przeglądarka nie wspiera Speech Synthesis.");
            return;
        }
        let utterance = new SpeechSynthesisUtterance(text);
        utterance.lang = lang;
        speechSynthesis.speak(utterance);
    }
};

window.downloadFileFromText = (filename, text) => {
    // Tworzymy blob z tekstem XML
    const blob = new Blob([text], { type: 'text/xml' });
    const link = document.createElement('a');
    link.download = filename;

    // Tworzymy URL do bloba
    link.href = URL.createObjectURL(blob);
    link.click();

    // Zwolnienie zasobów
    URL.revokeObjectURL(link.href);
};

window.saveFileWithPicker = async (content, mimeType) => {
    try {
        if (window.showSaveFilePicker) {
            const options = {
                types: [
                    {
                        description: 'Pliki XML',
                        accept: { 'application/xml': ['.xml'] },
                    },
                ],
            };
            const handle = await window.showSaveFilePicker(options);
            const writable = await handle.createWritable();
            await writable.write(new Blob([content], { type: mimeType }));
            await writable.close();
        } else {
            // Dla przeglądarek bez obsługi `showSaveFilePicker`
            const blob = new Blob([content], { type: mimeType });
            const link = document.createElement('a');
            link.download = 'data.xml';
            link.href = URL.createObjectURL(blob);
            link.click();
            URL.revokeObjectURL(link.href);
        }
    } catch (err) {
        console.error('Błąd zapisu pliku:', err);
    }
};

window.speechSynthesisInterop = {
    speak: (text, lang) => {
        return new Promise((resolve, reject) => {
            if (!window.speechSynthesis) {
                reject("Speech Synthesis API nie jest wspierane.");
                return;
            }

            const utterance = new SpeechSynthesisUtterance(text);
            utterance.lang = lang;

            utterance.onend = () => {
                console.log("Odtwarzanie zakończone.");
                resolve(); // Zwróć sukces, gdy mowa się zakończy
            };

            utterance.onerror = (event) => {
                console.error("Błąd podczas odtwarzania:", event);
                reject(event.error); // Zwróć błąd w przypadku problemów
            };

            window.speechSynthesis.speak(utterance);
        });
    }
};
