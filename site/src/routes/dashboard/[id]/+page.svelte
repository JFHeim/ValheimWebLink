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
	let refreshing = false;
	let bossesOpen = false;
	let modsOpen = false;
	let serverOffline = false;

	counter.subscribe((value) => {
		serverUrl = value.serverUrl;
		if (serverUrl != null) serverInfoPromise = data.getServerInfo;
	});

	const setServerInfo = (json) => {
		serverInfo = JSON.parse(JSON.stringify(json, null, 2));
		serverInfo.bosses = serverInfo.globalKeys
			.filter((key) => key.includes('defeated_'))
			.map((key) => key.replace('defeated_', ''))
			.map((key) => key.charAt(0).toUpperCase() + key.slice(1));
		counter.set_serverInfo(serverInfo);
		console.log('Set server info', serverInfo);
		document.title = `Dashboard | ${serverInfo.name} | Valheim Web Link`;
		data.title = `Dashboard | ${serverInfo.name}`;
	};

	onMount(() => {
		init();

		const interval = setInterval(() => {
			refreshing = true;
			serverInfo = null;
		}, 5000);

		return () => clearInterval(interval);
	});
	import { getToastStore } from '@skeletonlabs/skeleton';
	const toastStore = getToastStore();

	$: {
		if (refreshing === true && serverInfo) {
			toastStore.trigger({
				message: '✔ Refreshed',
				hideDismiss: true,
				timeout: 1000
			});
		}
	}
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
			<!-- <p>Loading server info...</p> -->
		{:then json}
			{setServerInfo(json)}
		{:catch error}
			{#if error.message == 'Failed to fetch' || error.message == 'TIMEOUT'}
				<aside class="alert variant-filled-warning mt-5" style="max-width: 400px;">
					<i class="fa-solid fa-triangle-exclamation text-4xl"></i>
					<div class="alert-message" data-toc-ignore>
						<h3 class="h3" data-toc-ignore>Warning</h3>
						<p>Server is offline or unreachable</p>
					</div>
					<div class="alert-actions">
						<button
							class="btn variant-filled-secondary"
							style="padding-left: 10px;"
							on:click={() => {
								refreshing = true;
								serverInfo = null;
								serverInfoPromise = data.getServerInfo;
							}}
						>
							<i class="fa-solid fa-refresh"></i>
							<p>Try again</p>
						</button>
					</div>
				</aside>
				<div class="hidden">
					{(serverOffline = true)}
				</div>
			{:else}
				<aside class="alert variant-filled-error mt-5">
					<i class="fa-solid fa-triangle-exclamation text-4xl"></i>
					<div class="alert-message" data-toc-ignore>
						<h3 class="h3" data-toc-ignore>Error</h3>
						<p>{error.message}</p>
					</div>
					<div class="alert-actions">
						<button
							class="btn variant-filled-secondary"
							style="padding-left: 10px;"
							on:click={() => {
								refreshing = true;
								serverInfo = null;
								serverInfoPromise = data.getServerInfo;
							}}
						>
							<i class="fa-solid fa-refresh"></i>
							<p>Try again</p>
						</button>
					</div>
				</aside>
				<div class="hidden">
					{(serverOffline = true)}
				</div>
			{/if}
		{/await}
	{/if}
{/if}

{#if serverInfo != null}
	<div class="info-panel">
		<div class="flex flex-row" style="align-items: flex-start;">
			<div>
				<header class="card-header h2" style="padding-top: 3px;">
					Manage <span class="t-bold"
						>{serverInfo.name === '' ? 'Your Server' : serverInfo.name}</span
					>
					<br />
					<hr />
				</header>

				<h3 class="h3 card-header mb-2">General info</h3>

				<ul>
					<li class="pl-4 pb-1">Game version: <span class="t-bold">{serverInfo.version}</span></li>
					<li class="pl-4">
						{#if serverInfo.playersCount > 0}🟢{:else}🟡{/if}
						Online players: <span class="t-bold"> {serverInfo.playersCount}</span>
					</li>
					{#if serverInfo.playersCount > 0}
						<ul>
							{#each serverInfo.players as player}
								<li class="pl-8">- <span class="t-bold">{player}</span></li>
							{/each}
						</ul>
					{/if}
					<div class="pb-1"></div>
					{#if serverInfo.banList.length > 0}
						<li class="pl-4">🛑 Banned players:</li>
						<ul>
							{#each serverInfo.banList as player}
								<li class="pl-8">- <span class="t-bold text-red-500">{player}</span></li>
							{/each}
						</ul>
					{:else}
						<li class="pl-4">💗 No banned players</li>
					{/if}
					<div class="pb-1"></div>
					{#if serverInfo.adminList.length > 0}
						<li class="pl-4">👑 Admins:</li>
						<ul>
							{#each serverInfo.adminList as player}
								<li class="pl-8">- <span class="t-bold text-gold">{player}</span></li>
							{/each}
						</ul>
					{:else}
						<li class="pl-4"><span style:color="yellow">⁉ </span>No admins</li>
					{/if}
					<div class="pb-1"></div>

					{#if serverInfo.time}
						<li class="pl-4">Server time: <span class="t-bold">{serverInfo.time}</span></li>
					{/if}
					{#if serverInfo.day}
						<li class="pl-4">Server day: <span class="t-bold">{serverInfo.day}</span></li>
					{/if}
					{#if serverInfo.timeOfDay}
						<li class="pl-4">It's <span class="t-bold">{serverInfo.timeOfDay}</span> on server</li>
					{/if}

					<div class="pb-2"></div>
					<li class="pl-4">
						<ul>
							<TreeView open={bossesOpen}>
								<TreeViewItem
									padding="p-1 w-fit pr-3 pl-2"
									on:toggle={(event) => (bossesOpen = event.detail.open)}
								>
									<span style:color="grey">📜 </span>Bosses killed
									<svelte:fragment slot="children">
										{#each serverInfo.bosses as bossName}
											<li class="pl-8">- <span class="t-bold">{bossName}</span></li>
										{/each}
									</svelte:fragment>
								</TreeViewItem>
							</TreeView>
						</ul>
					</li>

					<div class="pb-2"></div>
					<li class="pl-4">
						<ul>
							<TreeView>
								<TreeViewItem
									open={modsOpen}
									padding="p-1 w-fit pr-3 pl-2"
									on:toggle={(event) => (modsOpen = event.detail.open)}
								>
									<span style:color="grey">⚙ </span>Mods installed
									<svelte:fragment slot="children">
										{#each serverInfo.mods as modName}
											<li class="pl-8">- <span class="t-bold">{modName}</span></li>
										{/each}
									</svelte:fragment>
								</TreeViewItem>
							</TreeView>
						</ul>
					</li>
				</ul>
			</div>

			<button
				class="btn variant-filled-secondary"
				style="padding: 10px; border-radius: 100%; margin-top: 10px;"
				on:click={() => {
					refreshing = true;
					serverInfo = null;
					serverInfoPromise = data.getServerInfo;
				}}
			>
				<i class="fa-solid fa-refresh"></i>
			</button>
		</div>
	</div>
{:else if serverInfo === false}
	<header class="card-header h2" style="padding-top: 3px;">
		<div class="placeholder"></div>
		<br />
		<hr />
	</header>

	<div class="h3 card-header mb-2 placeholder"></div>

	<ul>
		<div class="placeholder"></div>
		<div class="placeholder"></div>
		<ul>
			{#each [1, 1] as player}
				<li class="pl-8">
					<div class="placeholder"></div>
				</li>
			{/each}
		</ul>
		<div class="pb-1"></div>
		<ul>
			{#each [1, 1] as player}
				<li class="pl-8">
					<div class="placeholder"></div>
				</li>
			{/each}
		</ul>
		<div class="pb-1"></div>
		<ul>
			{#each [1, 1] as player}
				<li class="pl-8">
					<div class="placeholder"></div>
				</li>
			{/each}
		</ul>
		<div class="pb-1"></div>

		<div class="placeholder"></div>
		<div class="placeholder"></div>

		<div class="pb-2"></div>
	</ul>
{/if}
