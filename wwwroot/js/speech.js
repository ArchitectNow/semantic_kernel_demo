window.startSpeechRecognition = function (azureKey, azureRegion, dotNetRef, timeoutSeconds) {
    const speechConfig = SpeechSDK.SpeechConfig.fromSubscription(azureKey, azureRegion);
    speechConfig.speechRecognitionLanguage = "en-US";
    const audioConfig = SpeechSDK.AudioConfig.fromDefaultMicrophoneInput();
    const recognizer = new SpeechSDK.SpeechRecognizer(speechConfig, audioConfig);

    let timeoutId;

    // Function to start or reset the timeout
    const resetTimeout = () => {
        if (timeoutId) clearTimeout(timeoutId);
        timeoutId = setTimeout(function () {
            recognizer.close(); // Stop recognition after timeout with no speech
            dotNetRef.invokeMethodAsync("OnSpeechTimeout");
        }, timeoutSeconds * 1000);
    };

    // Set initial timeout
    resetTimeout();

    // Clear and reset timeout when recognizing audio input
    recognizer.recognizing = (s, e) => {
        resetTimeout();
    };

    recognizer.recognizeOnceAsync(result => {
        clearTimeout(timeoutId); // Clear timeout once speech is fully recognized

        if (result.reason === SpeechSDK.ResultReason.RecognizedSpeech) {
            dotNetRef.invokeMethodAsync("OnSpeechRecognized", result.text);
        } else {
            dotNetRef.invokeMethodAsync("OnSpeechError", result.errorDetails);
        }
    });

    // Store recognizer globally for reset function access
    window.currentRecognizer = recognizer;
};

window.resetSpeechRecognition = function () {
    if (window.currentRecognizer) {
        window.currentRecognizer.close(); // Gracefully stop any active recognizer
        window.currentRecognizer = null; // Clear the reference to avoid memory leaks
    }
};

window.playAudio = function (base64Audio, dotNetRef) {
    const audio = new Audio("data:audio/wav;base64," + base64Audio);
    window.currentAudio = audio; // Store globally to allow stopping playback

    audio.onended = function () {
        dotNetRef.invokeMethodAsync("OnAudioPlaybackComplete");
    };

    audio.play();
};

window.stopAudioPlayback = function () {
    if (window.currentAudio) {
        window.currentAudio.pause();
        window.currentAudio.currentTime = 0; // Reset to start position if replayed later
    }
};