<script>
	import { counter } from '$lib/counter';
	import { onMount } from 'svelte';
	let fontSize = 'text-5xl'; // Default to desktop size
	let style = ''; // Default to desktop size
	let windowWidth = 0; // Default to desktop size

	const updateFontSize = () => {
		if (windowWidth >= 768) {
			fontSize = 'text-7xl';
			style = 'text-right';
		} else if (windowWidth >= 480) {
			fontSize = 'text-5xl';
			style = '';
		} else {
			fontSize = 'text-3xl';
			style = 'text-center';
		}
	};
	onMount(() => {
		updateFontSize();
	});

	$: updateFontSize();
</script>

<svelte:window bind:innerWidth={windowWidth} />

<h1 class={`header font-norse ${fontSize} ${style}`} style="transition: font-size 0.3s;">
	Welcome to Valheim Web Link
</h1>

<a class="bg-secondary-500 btn btn-sm" href="/dashboard">Go to dashboard</a>
