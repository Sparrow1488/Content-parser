// ПОИДЕЕ ЭТОТ КЛАСС ДОЛЖЕН БЫТЬ АБСТРАКТНЫМ!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
class SiteParser{
    searchFilter = {
        value: {
            tags: [],
            blockTags: []
        },
        get(){
            return this.value;
        }
    }
    parseUrl = {
        value: "http://site-url.com/currentPage",
        get(){
            return this.value;
        },
        set(setValue){
            this.value = setValue;
        },
        configurable: false
    }
    rootSiteUrl = {
        value: "http://site-url.com/",
        get(){
            return this.value;
        },
        set(setValue){
            this.value = setValue;
        },
        configurable: false
    }
    useFilter(searchFilter){
        if(searchFilter !== null || searchFilter !== undefined)
            this.searchFilter = searchFilter;
    }
    async parseContentLinksToList(parseUrl = this.parseUrl){
        let htmlLinksOnViews;
        let contentLinks;
        const remoteDocument = await this.#getRemoteHtml(parseUrl);
        if(remoteDocument !== null || remoteDocument !== undefined){
            const contentBlock = remoteDocument.querySelector(".content");
            htmlLinksOnViews = contentBlock.querySelectorAll(":scope > div > .thumb > a");
            const confirmedContent = this.#filterContent(htmlLinksOnViews);
            contentLinks = this.#getCurrentImageLink(confirmedContent);
        }
        else throw new Error("НЕ УДАЛОСЬ ПОЛУЧИТЬ ДОКУМЕНТ С УКАЗАННОГО САЙТА");
        return contentLinks;
    }
    async parseCurrentViewAsImage(itemUrlOfContent){
        let imageLink = "";
        imageLink = await this.#getImageLink(itemUrlOfContent);
        if(imageLink === null || imageLink === undefined)
            throw new Error(`ВОЗНИКЛА ОШИБКА ПРИ ПОЛУЧЕНИИ ССЫЛКИ НА ФАЙЛ РЕСУРСА ${itemUrlOfContent} ДЛЯ ТИПА IMAGE`);
        return imageLink;
    } 

    downloadCurrentFile(link) {
        try{
            chrome.downloads.download({
                url: link
            });
        }
        catch(error){
            console.log(`Failed to load file by link ${link}`);
        }
    }

    async #getRemoteHtml(url){
        try{
            const html = (await (await fetch(url)).text());
            const doc = new DOMParser().parseFromString(html, 'text/html');
            return doc;
        }
        catch(error){
            alert(`[ERROR]: Maybe, you forgot to use VPN (if you visit 'secret' web page)\nURL ${url}; HTML ${html}`);
        }
    }
    #getCurrentImageLink(linksOnViews){
        const currentImagesLinks = [];
        for (let i = 0; i < linksOnViews.length; i++) {
            const currentImageLink = this.rootSiteUrl + this.#healthLink(linksOnViews[i].href);
            currentImagesLinks.push(currentImageLink);
        }
        console.log("Array of links on view pages", currentImagesLinks);
        return currentImagesLinks;
    }
    #healthLink(badParsedLink){
        const correctLink = badParsedLink.split('/').pop();
        return correctLink;
    }
    async #getImageLink(currentViewLink){
        let imageLink = "";
        try{
            const rootImageDocument = await this.#getRemoteHtml(currentViewLink);
            const imageTag = rootImageDocument.querySelector("#image");
            imageLink = imageTag.src;
        }
        catch(err){
            console.error("[ERROR]: Received page wasn't has link on IMAGE!");
        }
        return imageLink;
    }
    #filterContent(contentList){
        let confirmedContent = [];
        console.log("INPUT CONTENT:", contentList.length);
        for (let i = 0; i < contentList.length; i++) {
            const contentTags = contentList[i].querySelector("img").alt.split(" ");
            const containsBlockTags = this.#containsBlockTags(contentTags);
            if(!containsBlockTags)
                confirmedContent.push(contentList[i]);
        }
        console.log("OUTPUT CONTENT:", confirmedContent.length);
        return confirmedContent;
    }
    #containsBlockTags(contentTags){
        let containsBlockTag = false;
        if(this.searchFilter && this.searchFilter.blockTags && arrayOfTags){
            for (let j = 0; j < this.searchFilter.blockTags.length; j++) {
                const blockTag = this.searchFilter.blockTags[j];
                if(contentTags.includes(blockTag)){
                    containsBlockTag = true;
                    console.error(`Content is blocked! It Contains blocked tags: ${blockTag}`);
                    break;
                }
            }
        }
        return containsBlockTag;
    }
}

class Rule34Parser extends SiteParser{
    constructor(){
        super();
        super.parseUrl = "https://rule34.xxx/index.php?page=post&s=list&tags=female";
        super.rootSiteUrl = "https://rule34.xxx/";
    }
}

class SearchFilter{
    tags = [];
    blockTags = [];
}

function getClientBlockTags(){
    const tagsList = [];
    const tags = document.querySelectorAll(".block-tags-list > span");
    for (const item of tags)
        if(item.innerText)
            tagsList.push(item.innerText);
    return tagsList;
}

function saveBlockTags(blockedTags) {
    chrome.storage.sync.set({"blockTags": blockedTags}, function() {
        console.log('Value is set to ', blockedTags);
    });
}

function getBlockTags() {
    return new Promise(function(resolve, reject){
        let tags;
        chrome.storage.sync.get(['blockTags'], function(result) {
            console.log('Value currently is ', result.blockTags);
            if(result.blockTags){
                resolve(result.blockTags);
            }
            else reject("ХУЕТА ЧЕЛ");
        });
    });
    
}

function displayBlockTagsRange(listTags) {
    const blockTagsList = document.querySelector(".block-tags-list");
    blockTagsList.innerHTML = "";
    listTags.forEach(tag => {
        insertBlockTag(tag);
    });
}

function insertBlockTag(tagValue) {
    if(tagValue && tagValue !== " "){
        const blockList = document.querySelector(".block-tags-list");
        const blockTag = document.createElement("SPAN");
        blockTag.innerText = tagValue;
        blockTag.classList.add("block-tag");
        blockTag.classList.add("text");
        blockTag.style.margin = "0px 5px 0px 0px"
        blockList.appendChild(blockTag);
    }
}

const downloadBtn = document.querySelector("#download");
downloadBtn.addEventListener("click", async function(){
    const parseLink = document.querySelector("#parseLink").value;
    let blockTags = getClientBlockTags();
    if(blockTags){
        saveBlockTags(blockTags);
    }
    blockTags = await getBlockTags();
    const rule34 = new Rule34Parser();
    rule34.parseUrl = parseLink;
    console.log(rule34.parseUrl);
    const filter = new SearchFilter();
    filter.blockTags = blockTags;
    rule34.useFilter(filter);
    const listOfContent = await rule34.parseContentLinksToList()
    let imageCounter = 0;
    console.log("START PARSE URL AND DOWNLOADING...");
    for (let i = 0; i < listOfContent.length; i++) {
        const imageUrl = await rule34.parseCurrentViewAsImage(listOfContent[i]);
        if(imageUrl !== null || imageUrl !== undefined){
            // rule34.downloadCurrentFile(imageUrl);
            imageCounter++;
        }
    }
    console.log("SUCCESS DOWNLOADED IMAGES IS", imageCounter);
});

let defBlockTagsMenuDisplay = document.querySelector(".block-tags-menu-container").style.display;
const openBlockTagsMenu = document.querySelector("#open-block-tags-panel");
openBlockTagsMenu.addEventListener("click", function(){
    const blockTagsMenu = document.querySelector(".block-tags-menu-container");
    if(blockTagsMenu.style.display === "none")
        blockTagsMenu.style.display = defBlockTagsMenuDisplay;
    else blockTagsMenu.style.display = "none";
});

const acceptNewBlockTagBtn = document.querySelector("#accept-new-block-tag-item");
acceptNewBlockTagBtn.addEventListener("click", function(){
    const newBlockTagItem = document.querySelector(".block-tags-menu");
    const newBlockTag = newBlockTagItem.value;
    insertBlockTag(newBlockTag);
    newBlockTagItem.value = "";
});



getBlockTags()
.then(function(blockTags){
    console.log("RECEIVED BLOCKED TAGS", blockTags);
    if(blockTags){
        displayBlockTagsRange(blockTags);
    }
})
.catch(err => console.error("ERROR LOADING TAGS", err));