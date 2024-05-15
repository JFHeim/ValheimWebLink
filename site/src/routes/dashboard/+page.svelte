<script>
	import { getImageLink } from '$lib/images';
	import { Avatar } from '@skeletonlabs/skeleton';

	export let data;
</script>

<!-- <div class="card 'bg-initial' p-4 flex justify-center items-center"><span>Minimal</span></div> -->
{#await data.servers}
	<div
		class="w-full text-token grid grid-cols-1 lg:grid-cols-3 md:grid-cols-2 sm:grid-cols-1 gap-4"
	>
		{#each [1, 1] as i}
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
		class="w-full text-token grid grid-cols-1 lg:grid-cols-3 md:grid-cols-2 sm:grid-cols-1 gap-4"
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
