import { sendRequest } from '$lib/utils';

export const load = async () => {
	const getServerInfo = async (url) => {
		if (!url) return null;
		const data = await sendRequest(`${url}/serverinfo`, 500);
		return data;
	};

	return {
		title: 'Dashboard',
		getServerInfo: getServerInfo
	};
};
