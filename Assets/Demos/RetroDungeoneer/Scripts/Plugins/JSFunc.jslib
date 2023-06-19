mergeInto(LibraryManager.library, {
    WebGLSyncFS: function () {
        FS.syncfs(false, function (err) {
            console.log('Error: syncfs failed!');
        });
    },
});