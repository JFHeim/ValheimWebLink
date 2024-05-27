<script>
	import { counter } from '$lib/counter.js';
	import { onMount } from 'svelte';

	export let data;
	let socket = null;
	let imageUrl = '';
	const init = () => {
		// counter.set_railId(3);
	};

	counter.subscribe((value) => {
		serverUrl = value.serverUrl;
		serverInfo = value.serverInfo;
	});

	let serverUrl = '';
	let serverInfo = null;
	onMount(() => {
		init();

		socket = data.createWebSocket();
		socket.onmessage = (event) => {
			let event_data = decodeURIComponent(event.data);
			if (event_data === 'ping') {
				socket.send('pong');
				return;
			}
			event_data = JSON.parse(event_data);
			imageUrl = `data:image/jpg;base64,${event_data.textureData}`;
			console.log(imageUrl);
			console.log(event_data.textureData);
		};
	});
</script>

{#if imageUrl}
	<img src={imageUrl} alt="Player screen" />
{:else}
	<p>Loading...</p>
{/if}
