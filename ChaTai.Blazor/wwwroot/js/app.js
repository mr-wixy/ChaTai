let windowEventListeners = [];
let isBottom = false;

function AddWindowWidthListener(objReference) {
    let eventListener = () => UpdateWindowWidth(objReference);
    window.addEventListener("resize", eventListener);
    windowEventListeners[objReference] = eventListener;
}

function RemoveWindowWidthListener(objReference) {
    window.removeEventListener("resize", windowEventListeners[objReference]);
}

function UpdateWindowWidth(objReference) {
    objReference.invokeMethodAsync("UpdateWindowWidth", window.innerWidth);
}

function AddScrollListener(objReference) {
    let eventListener = () => HandleScroll(objReference);
    windowEventListeners[objReference] = eventListener;
    var chat = document.getElementById("msg-box");
    chat.addEventListener("scroll", eventListener);
    windowEventListeners[objReference] = eventListener;
}

function RemoveScrollListener(objReference) {
    var chat = document.getElementById("msg-box");
    chat.removeEventListener("scroll", windowEventListeners[objReference]);
}

function HandleScroll(objReference) {
    var chat = document.getElementById("msg-box");
    const scrollTop = chat.scrollTop;
    if (Math.ceil(chat.scrollHeight - chat.scrollTop) === chat.clientHeight) {
        objReference.invokeMethodAsync("UpdateScroollPosition", true);
        isBottom = true;
    }
    else {
        if (isBottom == true) {
            objReference.invokeMethodAsync("UpdateScroollPosition", false);
            isBottom = false;
        }
    }
}

function ChangeTheme(value) {
    document.body.classList.remove("light");
    document.body.classList.remove("dark");

    if (value === "dark") {
        document.body.classList.add("dark");
    } else if (value === "light") {
        document.body.classList.add("light");
    }

    const themeColor = getComputedStyle(document.body)
        .getPropertyValue("--theme-color")
        .trim();
    const metaDescription = document.querySelector('meta[name="theme-color"]');
    metaDescription?.setAttribute("content", themeColor);
}

function ScrollToBottom() {
    var box = document.getElementById('msg-box');
    box.scrollTo({
        top: box.scrollHeight,
        behavior: "smooth"
    })
}
function highlightAll() {
    hljs.highlightAll()
}

function copyToClipboard(text) {
    navigator.clipboard.writeText(text);
}