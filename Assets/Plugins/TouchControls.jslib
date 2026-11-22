mergeInto(LibraryManager.library, {
    CursedAcademy_PrefersTouchControls: function () {
        var touchPoints = navigator.maxTouchPoints || 0;
        if (touchPoints === 0)
            return 0;

        var userAgent = navigator.userAgent;
        var userAgentData = navigator.userAgentData;
        if (/Android|iPad|iPhone|iPod/i.test(userAgent) ||
            (userAgentData && userAgentData.platform === "Android"))
            return 1;

        // iPadOS can report a desktop Mac user agent even with a trackpad connected.
        if (touchPoints > 1 && /Macintosh/i.test(userAgent))
            return 1;

        return window.matchMedia("(pointer: coarse) and (hover: none)").matches ? 1 : 0;
    }
});
