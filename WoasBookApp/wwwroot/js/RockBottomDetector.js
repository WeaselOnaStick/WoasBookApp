let scrollHandler = null;

function setupScrollListener(dotNetObj) {
    scrollHandler = () => {
        let hitRockBottom = window.outerHeight > document.documentElement.scrollHeight - window.pageYOffset
        if (hitRockBottom) {
            dotNetObj.invokeMethodAsync('OnHitRockBottom');
        }
    }
    window.addEventListener('scroll', scrollHandler)
}

function removeScrollListener() {
    if (scrollHandler) {
        window.removeEventListener('scroll', scrollHandler)
        scrollHandler = null;
    }
}