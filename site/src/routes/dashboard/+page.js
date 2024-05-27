import { sendRequest } from '$lib/utils';

export const load = () => {
	const isServerOnline = async ({ ip, port }) => {
		try {
			await sendRequest(`${ip}:${port}`, 1500);
		} catch (error) {
			console.error(`Catch error: ${error.message}`);
			return { isOnline: false };
		}
		return { isOnline: true, errorMessage: null };
	};
	return {
		isServerOnline: isServerOnline,
		title: 'Search & Login'
	};
};
