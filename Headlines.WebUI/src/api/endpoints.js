function getApiAddress() {
    let host = window.location.host.replace("www.", "")

    if (host.includes("localhost"))
        return "http://localhost:8083"

    return "https://api." + host
}

function getWsAddress() {
    let host = window.location.host.replace("www.", "")

    if (host.includes("localhost"))
        return "ws://localhost:8083"

    return "ws://api." + host
}

export default {
    HeadlineChanges: {
        GetTopUpvoted(take) {
            return take
                ? getApiAddress() + "/v1/HeadlineChanges/GetTopUpvoted?take=" + take
                : getApiAddress() + "/v1/HeadlineChanges/GetTopUpvoted"
        },
        GetSkipTake(skip, take) {
            return getApiAddress() + "/v1/HeadlineChanges/GetSkipTake?skip=" + skip + "&take=" + take
        },
        GetCount() {
            return getApiAddress() + "/v1/HeadlineChanges/GetCount"
        },
        GetByArticleIdSkipTake(articleId, skip, take) {
            return getApiAddress() + "/v1/HeadlineChanges/GetByArticleIdSkipTake?articleId=" + articleId + "&skip=" + skip + "&take=" + take
        },
        Upvote() {
            return getApiAddress() + "/v1/HeadlineChanges/Upvote"
        }
    },
    UserUpvotes: {
        Get(userToken) {
            return getApiAddress() + "/v1/UserUpvotes/Get?userToken=" + userToken
        }    
    },
    ArticleSources: {
        GetAll() {
            return getApiAddress() + "/v1/ArticleSources/GetAll"
        }
    },
    Articles: {
        GetById(id) {
            return getApiAddress() + "/v1/Articles/GetById?id=" + id
        },
        GetDetailById(id) {
            return getApiAddress() + "/v1/Articles/GetDetailById?id=" + id
        },
        GetSkipTake() {
            return getApiAddress() + "/v1/Articles/GetSkipTake"
        },
        RequestDetailScrape() {
            return getApiAddress() + "/v1/Articles/RequestDetailScrape"
        }
    },
    WebSocketServer: {
        GetAddress() {
            return getWsAddress()
        },
        Messages: {
            ListenToArticleDetailScrape(articleId) {
                return JSON.stringify({
                    actionName: "article-detail-scraped",
                    parameter: articleId
                })
            }
        }
    }
}