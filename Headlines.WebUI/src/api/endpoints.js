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

    return "wss://api." + host
}

export default {
    HeadlineChanges: {
        GetTopUpvoted(take) {
            return take
                ? getApiAddress() + "/v1/HeadlineChanges/TopUpvoted/Take/" + take
                : getApiAddress() + "/v1/HeadlineChanges/TopUpvoted"
        },
        GetSkipTake(skip, take) {
            return getApiAddress() + "/v1/HeadlineChanges/Skip/" + skip + "/Take/" + take
        },
        GetCount() {
            return getApiAddress() + "/v1/HeadlineChanges/Count"
        },
        Upvote() {
            return getApiAddress() + "/v1/HeadlineChanges/Upvote"
        }
    },
    UserUpvotes: {
        Get(userToken) {
            return getApiAddress() + "/v1/UserUpvotes/" + userToken
        }    
    },
    ArticleSources: {
        GetAll() {
            return getApiAddress() + "/v1/ArticleSources"
        }
    },
    Articles: {
        GetById(id) {
            return getApiAddress() + "/v1/Articles/" + id
        },
        GetDetailById(id) {
            return getApiAddress() + "/v1/Articles/" + id + "/Detail"
        },
        Search() {
            return getApiAddress() + "/v1/Articles/Search"
        },
        RequestDetailScrape() {
            return getApiAddress() + "/v1/Articles/RequestDetailScrape"
        },
        GetHeadlineChangesByArticleId(articleId, skip, take) {
            return getApiAddress() + "/v1/Articles/" + articleId + "/HeadlineChanges/Skip/" + skip + "/Take/" + take
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