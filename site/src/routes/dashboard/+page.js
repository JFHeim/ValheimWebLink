export const load = ({ fetch }) => {
	const isServerOnline = async ({ ip, port }) => {
		try {
			await fetch(`http://${ip}:${port}`);
		} catch (error) {
			const errorMessage = error.message.split(': ')[1];
			return { isOnline: false, errorMessage: errorMessage };
		}
		return { isOnline: true, errorMessage: null };
	};

	const checkServer = async ({ ip, port }) => {
		const isOnline = await isServerOnline(ip, port);
		if (isOnline) {
			const response = await fetch(`http://${ip}:${port}/serverinfo`);
			if (response.ok) return { errorMessage: null, data: await response.json() };
			else return { errorMessage: response.statusText, data: null };
		} else return { errorMessage: 'Server is offline', data: null };
	};

	const startServerCheck = ({ ip = 'localhost', port = 8080 }) => {
		checkServer(ip, port)
			.then((data) => {
				if (data === null) {
					console.log('Server is offline');
				} else {
					console.log(data);
				}
			})
			.catch((error) => {
				console.error(error);
			});
	};

	return {
		isServerOnline,
		startServerCheck: startServerCheck,
		title: 'Dashboard'
	};
};
