/** @type {import('./$types').PageLoad} */
export async function load({ params }) {
	return {
		// title: 'Dashboard',
		serverId: params.id
	};
}
