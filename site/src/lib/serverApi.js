export const serverApiById = async (id) => {
	console.log(`Looking for api for server ${id}`);
	const knownserversresponse = await fetch('http://localhost:5173/api/knownservers');
	const data = await knownserversresponse.json();

	const getServerInfo = async (ip, port) => {
		if (!ip || !port) return null;
		let response = null;
		try {
			response = await fetch(`http://${ip}:${port}/serverinfo`);
		} catch (error) {
			console.error(error);
		}
		if (response === null) return null;
		const data = await response.json();
		return data;
	};

	for (let i = 0; i < data.length; i++) {
		if (data[i].id == id)
			return { ...data[i], getServerInfo: getServerInfo(data[i].ip, data[i].port) };
	}

	return null;
};
