const baseRule34Url = "https://rule34.xxx/";

const rule34Url = document.querySelector("#inputLink"); // TODO : добавить возможность ...
console.log(rule34Url);
// const rule34Url = "";

async function getRemoteHtml(url){
    try{
        const html = (await (await fetch(url)).text());
        const doc = new DOMParser().parseFromString(html, 'text/html');
        return doc;
    }
    catch(error){
        alert("[ERROR]: Maybe, you forgot to use VPN (if you visit 'secret' web page)");
    }
}

function getCurrentImagesLinks(contentHtmlLinks){
    const currentImagesLinks = [];
    for (let i = 0; i < contentHtmlLinks.length; i++) {
        const currentImageLink = baseRule34Url + healthLink(contentHtmlLinks[i].href);
        currentImagesLinks.push(currentImageLink);
    }
    console.log(currentImagesLinks);
    return currentImagesLinks;
}

function healthLink(brakeLink){
    const goodLink = brakeLink.split('/').pop();
    return goodLink;
}

async function getDownloadFileLink(currentPageLink){
    let imageLink = "";
    try{
        // console.log("[Load image] Current page", currentPageLink);
        const rootImageDocument = await getRemoteHtml(currentPageLink);
        console.log(rootImageDocument);
        const imageTag = rootImageDocument.querySelector("#image");
        imageLink = imageTag.src;
        return imageLink;
    }
    catch(err){
        console.error("[ERROR]: Received page wasn't has link on IMAGE!");
    }
}

async function parseRule34Content(siteHomeDocument){
    try {
        const contentPanel = siteHomeDocument.querySelector(".content");
        console.log("Content", contentPanel);
        const contentHtmlLinks = contentPanel.querySelectorAll(":scope > div > .thumb > a");
        console.log("Content links", contentHtmlLinks);
        const currentImagePages = getCurrentImagesLinks(contentHtmlLinks);
        // save first 10 files
        for (let i = 0; i < 9; i++) {
            const downloadLink = await getDownloadFileLink(currentImagePages[i]);
            if(downloadLink !== undefined && downloadLink !== null){
                console.log("Download link", downloadLink);
                downloadURL(downloadLink);
            }
        }
    }
    catch(error){
        console.error("[Full party]Script ERROR:", error);
    }
}

// function getLinksOnImages(currentImagesPages){
//     console.log(currentImagesPages);
//     for (let i = 0; i < currentImagesPages.length; i++) {
        
//     }
// }

function downloadURL(link) {
    console.log("Download image by URL: ", link);
    chrome.downloads.download({
        url: link
    });
}

getRemoteHtml(rule34Url)
.then(async function(remoteDoc){
    console.log("Remote converted document", remoteDoc);
    await parseRule34Content(remoteDoc);
});