/** @type {import('./$types').LayoutLoad} */
export async function load({ params }) {
	return {
		title: 'Dashboard',
		serverId: params.id
	};
}
