<script>
	import { getImageLink } from '$lib/images';
	import { Avatar } from '@skeletonlabs/skeleton';
	import { onMount } from 'svelte';

	export let data;
	let columCount = 1;

	const updateFontSize = () => {
		if (window.innerWidth >= 1800) columCount = 5;
		else if (window.innerWidth >= 1400) columCount = 4;
		else if (window.innerWidth >= 1000) columCount = 3;
		else if (window.innerWidth >= 710) columCount = 2;
		else columCount = 1;
	};
	onMount(() => {
		updateFontSize();
		window.addEventListener('resize', updateFontSize);
		return () => window.removeEventListener('resize', updateFontSize);
	});
</script>

<!-- <div class="card 'bg-initial' p-4 flex justify-center items-center"><span>Minimal</span></div> -->
{#await data.servers}
	<div
		class="w-full text-token grid gap-4 grid-cols-1 lg:grid-cols-3 md:grid-cols-2 sm:grid-cols-1"
	>
		{#each [0, 0] as { }}
			<div class="card 'bg-initial' card-hover overflow-hidden">
				<header>
					<div
						class="placeholder animate-pulse bg-black/50 w-full aspect-[21/9]"
						style="height: 165px;"
					/>
				</header>
				<div class="p-4 space-y-4">
					<div class="flex justify-start">
						<div class="flex-auto flex justify-between items-right">
							<div class="placeholder animate-pulse"></div>
							<div class="placeholder animate-pulse"></div>
						</div>
					</div>
					<article>
						<div class="placeholder animate-pulse" />
						<div class="placeholder animate-pulse" />
						<div class="placeholder animate-pulse" />
					</article>
				</div>
			</div>
		{/each}
	</div>
{:then servers}
	<div
		class={`w-full text-token grid gap-4`}
		style={`grid-template-columns: repeat(${columCount}, minmax(0, 1fr));`}
	>
		{#each servers as server}
			<a class="card 'bg-initial' card-hover overflow-hidden" href="/dashboard/{server.id}">
				<header>
					<img
						src={getImageLink({ id: 'vjUokUWbFOs', w: 400, h: 175 })}
						class="bg-black/50 w-full aspect-[21/9]"
						alt="Post"
					/>
				</header>
				<div class="p-4 space-y-4">
					<div class="flex justify-start">
						<div class="flex-auto flex justify-between items-right">
							<h3 class="h3" data-toc-ignore>{server.name}</h3>
							<small class="">{server.statusText}</small>
						</div>
					</div>
					<article>
						<p class="text-clamp-3">
							{server.description}
						</p>
					</article>
				</div>
			</a>
		{/each}
	</div>
{:catch error}
	<p class="text-error">{error.message}</p>
{/await}
