import { serverApiById } from '$lib/serverApi.js';

export const load = async ({ params }) => {
	return {
		title: 'Dashboard',
		serverId: params.id.split(' ')[0],
		serverApi: serverApiById(params.id.split(' ')[0])
	};
};
