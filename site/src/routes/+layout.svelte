<script>
	import '../app.postcss';
	export let data;

	// Highlight JS
	import hljs from 'highlight.js/lib/core';
	import 'highlight.js/styles/github-dark.css';
	import { AppBar, storeHighlightJs } from '@skeletonlabs/skeleton';
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
	storePopup.set({ computePosition, autoUpdate, flip, shift, offset, arrow });

	onMount(() => {
		console.log(`main layout mounted`);

		// Set document title
		if (document.title === $page.url.pathname) document.title = 'Valheim Web Link';
	});
</script>

<div class="flex-row absolute h-5 top-4 w-full flex justify-center" style="height: 0">
	<p class="text-xl text-center" style="width: fit-content; height: fit-content">
		{$page.data.title}
	</p>
</div>
<AppBar>
	<svelte:fragment slot="lead">
		<a href="/" class="btn btn-sm variant-filled" data-sveltekit-preload-data> Home </a>
	</svelte:fragment>

	<svelte:fragment slot="trail">
		{#if $page.url.pathname !== '/dashboard'}
			<a href="/dashboard" class="btn btn-sm variant-filled" data-sveltekit-preload-data>
				Dashboard
			</a>
		{/if}
	</svelte:fragment>
</AppBar>

<div class="px-4 py-2">
	<slot />
</div>
