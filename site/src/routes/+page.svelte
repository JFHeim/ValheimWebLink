<script>
	import { onMount } from 'svelte';
	let fontSize = 'text-5xl'; // Default to desktop size
	let style = ''; // Default to desktop size

	onMount(() => {
		// Adjust font size based on screen width
		const tabletBreakpoint = 768;
		const mobileBreakpoint = 480;

		const updateFontSize = () => {
			if (window.innerWidth >= tabletBreakpoint) {
				fontSize = 'text-7xl'; // Desktop size
				style = 'text-right';
			} else if (window.innerWidth >= mobileBreakpoint) {
				fontSize = 'text-5xl'; // Tablet size
				style = '';
			} else {
				fontSize = 'text-3xl'; // Mobile size
				style = 'text-center';
			}
		};

		updateFontSize();
		window.addEventListener('resize', updateFontSize);

		return () => window.removeEventListener('resize', updateFontSize);
	});
</script>

<h1 class={`header font-norse ${fontSize} ${style}`} style="transition: font-size 0.3s;">
	Welcome to Valheim Web Link
</h1>

<a class="bg-secondary-500 btn btn-sm" href="/dashboard">Go to dashboard</a>
