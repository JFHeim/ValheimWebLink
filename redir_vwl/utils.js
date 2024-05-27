/** @param {string} url
 * @param {number} ms
 * @returns {Promise<Response>} */

export function timeoutFetch(ms, url) {
    return new Promise((resolve, reject) => {
        const timer = setTimeout(() => {
            reject(new Error('Gateway Timeout: External server did not respond'));
        }, ms);

        const promise = fetch(url);
        promise
            .then((value) => {
                clearTimeout(timer);
                resolve(value);
            })
            .catch((reason) => {
                clearTimeout(timer);
                reject(reason);
            });
    });
}

export function timeout(ms, promise) {
    return new Promise((resolve, reject) => {
        const timer = setTimeout(() => {
            reject(new Error('Gateway Timeout: External server did not respond'));
        }, ms);

        promise
            .then((value) => {
                clearTimeout(timer);
                resolve(value);
            })
            .catch((reason) => {
                clearTimeout(timer);
                reject(reason);
            });
    });
}
