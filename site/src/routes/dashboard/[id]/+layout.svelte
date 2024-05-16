<script>
	import { onMount, setContext } from 'svelte';
	export let data;
	const { serverApi } = data;

	onMount(() => {
		console.log(`dashboard[id] layout mounted`);
	});
</script>

{#await serverApi}
	<p>Loading server api...</p>
{:then serverApi}
	{#if serverApi == null}
		<aside class="alert variant-filled-error">
			<i class="fa-solid fa-triangle-exclamation text-4xl"></i>
			<div class="alert-message" data-toc-ignore>
				<h3 class="h3" data-toc-ignore>Server not found</h3>
				<p>Server with id '{data.serverId}' not found</p>
			</div>
		</aside>
	{:else}
		{setContext('serverApi', serverApi)}
		{console.log('serverApi loaded', serverApi)}
		{#await serverApi.getServerInfo()}
			<p>Loading server info...</p>
		{:then serverInfo}
			<pre class="pre">{serverInfo}</pre>
			{setContext('serverInfo', serverInfo)}
			{console.log('serverInfo loaded', serverInfo)}
			{(document.title = `${serverInfo.serverName} - Valheim Web Link`)}
			<slot />
		{:catch error}
			<p class="error">{error.message}</p>
		{/await}
	{/if}
{:catch error}
	<p class="error">{error.message}</p>
{/await}
