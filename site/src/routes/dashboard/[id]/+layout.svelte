<script>
	import { onMount } from 'svelte';

	/** @type {import('./$types').LayoutData} */
	export let data;

	onMount(() => {
		console.log(`dashboard[id] layout mounted`);
		document.title = `${data.serverId} - Valheim Web Link`;
	});
</script>

<h2 class="h2 text-center">Welcome to {data.serverId} dashboard</h2>
<slot />
