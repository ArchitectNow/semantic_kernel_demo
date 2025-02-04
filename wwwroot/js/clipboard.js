window.copyToClipboard = (text) => {
    navigator.clipboard.writeText(text).then(function () {
        console.log('Text copied to clipboard');
    }, function (err) {
        console.error('Could not copy text: ', err);
    });
};
