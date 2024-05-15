export const load = async ({ fetch }) => {
	const fetchServerInfo = async () => {
		const response = await fetch('http://localhost:8080/serverinfo');
		const data = await response.json();
		return data;
	};

	return {
		serverInfo: fetchServerInfo()
	};
};
