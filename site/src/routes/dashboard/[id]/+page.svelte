<script>
	import { TreeView, TreeViewItem, RecursiveTreeView } from '@skeletonlabs/skeleton';
	import { page } from '$app/stores';
	import { counter } from '$lib/counter';
	import { onMount } from 'svelte';
	export let data;
	const init = () => {
		counter.set_railId(1);
		counter.set_serverUrl($page.params.id);
	};

	let serverUrl = '';
	let serverInfoPromise = null;
	let serverInfo = null;

	counter.subscribe((value) => {
		serverUrl = value.serverUrl;
		if (serverUrl != null) serverInfoPromise = data.getServerInfo;
	});

	const setServerInfo = (json) => {
		serverInfo = JSON.parse(JSON.stringify(json, null, 2));
		counter.set_serverInfo(serverInfo);
		console.log('Set server info', serverInfo);
		document.title = `Dashboard | ${serverInfo.name} | Valheim Web Link`;
		data.title = `Dashboard | ${serverInfo.name}`;
	};

	onMount(() => init());
</script>

{#if serverUrl && serverInfo == null}
	{#if serverUrl == 'null' || serverInfoPromise == null}
		<aside class="alert variant-filled-warning mt-5">
			<i class="fa-solid fa-triangle-exclamation text-4xl"></i>
			<div class="alert-message" data-toc-ignore>
				<h3 class="h3" data-toc-ignore>Warning</h3>
				<p>No server info provided</p>
			</div>
			<div class="alert-actions">
				<button class="btn-icon variant-filled" on:click={() => counter.set_railId(0)}>
					<i class="fa-solid fa-xmark"></i>
				</button>
			</div>
		</aside>
	{:else}
		{#await serverInfoPromise(serverUrl)}
			<p>Loading server info...</p>
		{:then json}
			{setServerInfo(json)}
		{:catch error}
			{@debug error}
			<p>Error: {error.message}</p>
		{/await}
	{/if}
{/if}

{#if serverInfo != null}
	<p>Server name: {serverInfo.name}</p>

	<TreeView>
		<TreeViewItem>
			(item 1)
			<svelte:fragment slot="children">
				<TreeViewItem>
					(Child 1)
					<svelte:fragment slot="children">
						<TreeViewItem>(Child of Child 1)</TreeViewItem>
						<TreeViewItem>(Child of Child 2)</TreeViewItem>
					</svelte:fragment>
				</TreeViewItem>
				<TreeViewItem>(Child 2)</TreeViewItem>
			</svelte:fragment>
		</TreeViewItem>
		<TreeViewItem>(item 2)</TreeViewItem>
	</TreeView>
	<!-- <pre class="pre">{JSON.stringify(serverInfo, null, 2)}</pre> -->
{/if}
