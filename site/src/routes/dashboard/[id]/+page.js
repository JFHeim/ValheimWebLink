export const load = async () => {
	function timeout(ms, promise) {
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

	const getServerInfo = async (url) => {
		if (!url) return null;
		const response = await timeout(500, fetch(`http://${url}/serverinfo`));
		const data = await response.json();
		return data;
	};

	return {
		title: 'Dashboard',
		getServerInfo: getServerInfo
	};
};
