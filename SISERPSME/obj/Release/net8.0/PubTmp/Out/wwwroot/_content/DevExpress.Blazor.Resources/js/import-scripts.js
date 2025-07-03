const loadedResources = {};

async function wrapEvalEndCall(callback) {
    const oldeval = window.eval;
    window.eval = (x) => {
        window.eval = oldeval;
        if(x === "this") return window;
    }
    await callback();
}

async function importAllResources(resources) {
    for(let i = 0;i < resources.length;i++) {
        try {
            if(!loadedResources[resources[i]]) {
                let resourcePath = resources[i][0] === "." ? "../../../" + resources[i].substring(2) : resources[i]; // take a module from root
                if(resourcePath.indexOf("knockout") !== -1) {
                    await wrapEvalEndCall(async () => await import(resourcePath));
                } else {
                    await import(resourcePath);
                }
                loadedResources[resources[i]] = true;
            }
        } catch { }
    }
}

const loaderPromise = (async function() {
    const resources = [...document.getElementsByTagName("dxbl-resource-script")];
    if(resources.length > 0) {
        await importAllResources(resources.map(x => x.getAttribute("src")));
    }
})()

window["_dx_loader_promise"] = loaderPromise;

window["_dx_import_resources"] = async (params) => {
    await loaderPromise;
    await importAllResources(params);
}

export default await loaderPromise;
