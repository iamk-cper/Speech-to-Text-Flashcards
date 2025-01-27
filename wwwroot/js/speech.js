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
