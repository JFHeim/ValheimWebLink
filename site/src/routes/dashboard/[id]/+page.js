export const load = async () => {
	const getServerInfo = async (url) => {
		if (!url) return null;
		const response = await fetch(`http://${url}/serverinfo`);
		const data = await response.json();
		return data;
	};

	return {
		title: 'Dashboard',
		getServerInfo: getServerInfo
	};
};
