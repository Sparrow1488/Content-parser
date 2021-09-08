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
            contentLinks = this.#getCurrentImageLink(htmlLinksOnViews);
        }
        else throw new Error("НЕ УДАЛОСЬ ПОЛУЧИТЬ ДОКУМЕНТ С УКАЗАННОГО САЙТА");
        return contentLinks;
    }
    async parseCurrentViewAsImage(itemUrlOfContent){
        let imageLink = "";
        console.log("Link on post", itemUrlOfContent);
        imageLink = await this.#getImageLink(itemUrlOfContent);
        console.log("IMAGE LINK: ", imageLink);
        if(imageLink === null || imageLink === undefined)
            throw new Error(`ВОЗНИКЛА ОШИБКА ПРИ ПОЛУЧЕНИИ ССЫЛКИ НА ФАЙЛ РЕСУРСА ${itemUrlOfContent} ДЛЯ ТИПА IMAGE`);
        return imageLink;
    } 

    downloadCurrentFile(link) {
        try{
            console.log("Download image by URL: ", link);
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
            console.log("Root view doc", rootImageDocument);
            const imageTag = rootImageDocument.querySelector("#image");
            imageLink = imageTag.src;
        }
        catch(err){
            console.error("[ERROR]: Received page wasn't has link on IMAGE!");
        }
        return imageLink;
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

const rule34 = new Rule34Parser();
const filter = new SearchFilter();
// filter.tags =  ["fff", "asd", "1234"];
filter.blockTags =  ["block-1", "block-2", "block-3"];
rule34.useFilter(filter);
rule34.parseContentLinksToList()
.then(async function(listOfContent) {
    console.log("List of current views links", listOfContent);
    for (let i = 0; i < listOfContent.length; i++) {
        const imageUrl = await rule34.parseCurrentViewAsImage(listOfContent[i]);
        console.log("Image URL", imageUrl);
        if(imageUrl !== null || imageUrl !== undefined)
            rule34.downloadCurrentFile(imageUrl);
    }
});
