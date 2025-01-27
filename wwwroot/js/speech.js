﻿window.speechRecognitionInterop = {
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
                resolve();
            };

            utterance.onerror = (event) => {
                console.error("Błąd podczas odtwarzania:", event);
                reject(event.error);
            };

            window.speechSynthesis.speak(utterance);
        });
    }
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
            // Dla starszych przeglądarek
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
