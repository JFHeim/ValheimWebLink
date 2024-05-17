<script>
	import '../app.postcss';
	export let data;
	/** @type {number}*/
	export let railId;

	// Highlight JS
	import hljs from 'highlight.js/lib/core';
	import 'highlight.js/styles/github-dark.css';
	import { AppBar, AppRail, AppRailTile, storeHighlightJs } from '@skeletonlabs/skeleton';
	import xml from 'highlight.js/lib/languages/xml'; // for HTML
	import css from 'highlight.js/lib/languages/css';
	import javascript from 'highlight.js/lib/languages/javascript';
	import typescript from 'highlight.js/lib/languages/typescript';

	hljs.registerLanguage('xml', xml); // for HTML
	hljs.registerLanguage('css', css);
	hljs.registerLanguage('javascript', javascript);
	hljs.registerLanguage('typescript', typescript);
	storeHighlightJs.set(hljs);

	// Floating UI for Popups
	import { computePosition, autoUpdate, flip, shift, offset, arrow } from '@floating-ui/dom';
	import { storePopup } from '@skeletonlabs/skeleton';
	import { page } from '$app/stores';
	import { onMount } from 'svelte';
	import { Icon } from 'svelte-icons-pack';
	import { AiOutlineSearch, AiOutlineInfoCircle } from 'svelte-icons-pack/ai';
	import { BiCube, BiWorld } from 'svelte-icons-pack/bi';
	storePopup.set({ computePosition, autoUpdate, flip, shift, offset, arrow });

	// Font Awesome
	import '@fortawesome/fontawesome-free/css/fontawesome.css';
	import '@fortawesome/fontawesome-free/css/brands.css';
	import '@fortawesome/fontawesome-free/css/solid.css';
	import { counter } from '$lib/counter';

	let showAppRail = true;
	const checkAppRail = () => {
		showAppRail = $page.url.pathname.includes('/dashboard') && window.innerWidth >= 620;
	};

	onMount(() => {
		console.log(`main layout mounted`);
		// Set document title
		if (document.title === $page.url.pathname) document.title = 'Valheim Web Link';
		const appBar = document.getElementsByClassName('app-bar')[0];
		// @ts-ignore
		appBar.style.paddingLeft = '19px';
		// @ts-ignore
		appBar.style.maxHeight = '58px';
		// @ts-ignore
		document.getElementsByClassName('app-rail')[0].style.maxWidth = '71px';

		checkAppRail();
	});

	counter.subscribe((value) => {
		railId = value.railId;
	});

	$: {
		counter.set_railId(railId);
	}
</script>

<!-- TODO: checkAppRail on route change, not on click -->
<svelte:window on:resize={checkAppRail} on:keydown={checkAppRail} on:click={checkAppRail} />

<div class="flex-row absolute h-5 top-4 w-full flex justify-center" style="height: 0">
	<p class="text-xl text-center" style="width: fit-content; height: fit-content">
		{$page.data.title}
	</p>
</div>
<AppBar padding="p-4">
	<svelte:fragment slot="lead">
		<a href="/" class="btn btn-sm variant-filled" title="Home" data-sveltekit-preload-data>
			<i class="fa-solid fa-house" />
		</a>
	</svelte:fragment>
	<!-- <svelte:fragment slot="trail">
		{#if $page.url.pathname !== '/dashboard'}
			<a href="/dashboard" class="btn btn-sm variant-filled" data-sveltekit-preload-data>
				Dashboard
			</a>
		{/if}
	</svelte:fragment> -->
</AppBar>
<!-- TODO: fix that bullshit 'height: 94%' 💀 -->
<div class="flex flex-row" style="height: 94%;">
	{#if showAppRail === true}
		<AppRail class="h-full">
			<AppRailTile bind:group={railId} name="" value={0} title="Server search">
				<svelte:fragment slot="lead">
					<div class="centered h-fit w-fit">
						<Icon src={AiOutlineSearch} size="1.9em" />
					</div>
				</svelte:fragment>
				<span> </span>
			</AppRailTile>
			<AppRailTile bind:group={railId} name="Info" value={1} title="Server info">
				<svelte:fragment slot="lead">
					<div class="centered h-fit w-fit">
						<Icon src={AiOutlineInfoCircle} size="1.9em" />
					</div>
				</svelte:fragment>
			</AppRailTile>
			<AppRailTile bind:group={railId} name="World" value={2} title="World info">
				<svelte:fragment slot="lead">
					<div class="centered h-fit w-fit">
						<Icon src={BiWorld} size="1.9em" />
					</div>
				</svelte:fragment>
			</AppRailTile>
			<AppRailTile bind:group={railId} name="ObjectControl" value={3} title="Object Control">
				<svelte:fragment slot="lead">
					<div class="centered h-fit w-fit">
						<Icon src={BiCube} size="1.9em" />
					</div>
				</svelte:fragment>
			</AppRailTile>
		</AppRail>
	{/if}
	<div class="limited-content-screen">
		<div class="px-4 py-2">
			<slot />
		</div>
	</div>
</div>
<slot name="after-content" />
