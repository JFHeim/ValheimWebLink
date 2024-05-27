import { timeoutFetch } from '$lib/utils';

export const load = () => {
	const isServerOnline = async ({ ip, port }) => {
		try {
			await timeoutFetch(`http://${ip}:${port}`, 500);
		} catch (error) {
			console.error(`Catch error: ${error.message}`);
			// const errorMessage = error.message.includes(': ')
			// 	? error.message.split(': ')[1]
			// 	: error.message;
			return {
				isOnline: false
				// errorMessage: errorMessage
			};
		}
		return { isOnline: true, errorMessage: null };
	};

	const checkLogin = async ({ login, password }) => {
		return {
			error: null,
			loginData: {}
		};
	};

	return {
		isServerOnline,
		checkLogin,
		title: 'Search & Login'
	};
};
