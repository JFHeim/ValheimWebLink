// export const load = async ({ fetch }) => {
export const load = async () => {
	const fetchServers = async () => {
		const statusText = (status = 'Offline') => (status === 'Online' ? '🟢 Online' : '🔴 Offline');
		const response = await fetch('http://localhost:5173/api/knownservers');
		let data = await response.json();

		for (let i = 0; i < data.length; i++) {
			data[i] = { ...data[i], statusText: statusText(data[i].status) };
		}
		return data;
	};

	return {
		title: 'Servers List',
		servers: fetchServers()
	};
};
