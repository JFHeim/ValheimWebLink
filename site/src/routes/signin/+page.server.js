/* eslint-disable no-unused-vars */
import { sendRequest, timeout } from '$lib/utils.js';
import { redirect } from '@sveltejs/kit';

export const load = async (event) => {
	const formData = await event.request.formData();
	const sessionId = event.cookies.get('sessionId');
	const serverUrl = formData.get('serverUrl');

	if (sessionId) {
		console.log('Session exists,', sessionId);
		throw redirect(303, '/dashboard/' + serverUrl);
	}

	// const checkLogin = async ({ login, password }) => {
	// 	return {
	// 		error: null,
	// 		loginData: {
	// 			username: login,
	// 			role: 'full-admin'
	// 		}
	// 	};
	// };

	return { title: 'Login' };
};

export const actions = {
	default: async (event) => {
		const formData = await event.request.formData();
		const username = formData.get('username');
		const password = formData.get('password');
		const serverUrl = formData.get('serverUrl');
		const body = await JSON.stringify({ username, password });

		const res = await timeout(
			2000,
			fetch('https://api.just-a-frogger.ru/signin', {
				body,
				method: 'POST',
				headers: { 'Content-Type': 'application/json' }
			})
		);

		if (res.ok) {
			const sessionId = res.headers.get('Authorization');
			event.cookies.set('sessionId', sessionId?.split('Bearer ')[1] ?? '', {
				path: '/'
			});

			console.log('Session created', sessionId);
			throw redirect(303, '/dashboard');
		}
	}
};
