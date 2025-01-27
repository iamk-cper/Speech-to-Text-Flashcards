window.playSound = (filePath) => {
    const audio = new Audio(filePath);
    audio.play();
};