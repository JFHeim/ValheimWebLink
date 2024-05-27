/**
 * @param {number} ms
 * @param {string} url
 * @returns {Promise<Response>}
 * */
export function timeoutFetch(url, ms) {
	return new Promise((resolve, reject) => {
		const timer = setTimeout(() => {
			reject(new Error('TIMEOUT'));
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

/**
 * @param {number | undefined} ms
 * @param {Promise<any>} promise
 * @returns {Promise<Response>}
 */
export function timeout(promise, ms) {
	return new Promise((resolve, reject) => {
		const timer = setTimeout(() => {
			reject(new Error('TIMEOUT'));
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

/**
 * @param {string} input
 * @returns {Promise<Response>}>}
 */
export async function hash(input) {
	const fetchUrl = new URL(`https://api.just-a-frogger.ru/hash`);
	fetchUrl.searchParams.set('input', input);
	return await (await timeoutFetch(fetchUrl.href, 1000)).json();
}

/**
 * @param {string} url
 * @param {number} timeout
 * @returns {Promise<{}>}
 */
export async function sendRequest(url, timeout) {
	const fetchUrl = new URL(`https://api.just-a-frogger.ru/redir`);
	fetchUrl.searchParams.set('urlToFetch', url);
	fetchUrl.searchParams.set('timeout', timeout.toString());
	return await (await timeoutFetch(fetchUrl.href, timeout)).json();
}
