window.scrollToEnd = (element) => {
    if (element) {
        element.scrollTo({top: element.scrollHeight, behavior: 'smooth'});
    }
}

// function submitOnEnter(element) {
//     element.addEventListener('keydown', event => {
//         if (event.key === 'Enter') {
//             event.target.dispatchEvent(new Event('change'));
//             event.target.closest('form').dispatchEvent(new Event('submit'));
//         }
//     });
// }

function adjustTextareaHeight(element) {
    element.style.height = "auto";
    element.style.height = (element.scrollHeight) + "px";
}

function resetMenuPosition() {
    var elements = document.querySelectorAll(".mud-popover");
    elements.forEach(function (element) {
        element.style.top = "";
        element.style.left = "";
        element.style.transform = "";
    });
}