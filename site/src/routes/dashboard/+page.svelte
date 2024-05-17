<script>
	import { counter } from '$lib/counter.js';
	import { onMount } from 'svelte';
	export let data;

	/**@type {boolean | null}*/
	let checking = null;
	/**@type {boolean | null}*/
	let isOnline = null;
	/**@type {string | null}*/
	let errorMessage = null;

	const init = () => {
		counter.set_railId(0);
	};
	onMount(() => {
		init();

		const ipInput = document.getElementsByName('ip')[0];
		const portInput = document.getElementsByName('port')[0];

		if (counter.get().serverUrl != null && counter.get().serverUrl != 'null') {
			ipInput.setAttribute('value', counter.get().serverUrl.split(':')[0]);
			portInput.setAttribute('value', counter.get().serverUrl.split(':')[1]);
		}
	});

	const startServerCheck = async () => {
		isOnline = null;
		checking = null;
		errorMessage = null;
		const formData = {
			// @ts-ignore
			ip: document.getElementsByName('ip')[0].value,
			// @ts-ignore
			port: document.getElementsByName('port')[0].value
		};

		const validateIP = () => {
			const ipPattern =
				/^(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$/;
			return formData.ip === 'localhost' || ipPattern.test(formData.ip);
		};

		const validatePort = () => {
			const portPattern = /^[0-9]{1,5}$/;
			return portPattern.test(formData.port);
		};

		if (!formData.ip) {
			errorMessage = 'ip is required';
			return;
		}
		if (!validateIP()) {
			errorMessage = 'ip is not valid';
			return;
		}
		if (!formData.port) {
			errorMessage = 'port is required';
			return;
		}
		if (!validatePort()) {
			errorMessage = 'port is not valid';
			return;
		}
		console.log(formData);
		checking = true;
		const result = await data.isServerOnline(formData);
		isOnline = result.isOnline;
		errorMessage = result.errorMessage;
		console.log({ isOnline, errorMessage });
		checking = false;

		if (errorMessage === null && isOnline === true) {
			counter.set_serverUrl(`${formData.ip}:${formData.port}`);
			counter.set_railId(1);
		}
	};
</script>

<label class="label centered-rl w-fit">
	<p class="w-fit centered-rl">Search</p>
	<div class="w-fit centered-rl flex flex-row gap-2">
		<div class="centered-rl input-group input-group-divider flex limited">
			<input
				name="ip"
				class="input-number"
				placeholder="ip"
				type="text"
				on:keyup={() => (errorMessage = null)}
			/>
			<input
				name="port"
				class="input-number"
				placeholder="port"
				on:keyup={() => (errorMessage = null)}
			/>
		</div>
		<button class="centered btn variant-filled-secondary" on:click={startServerCheck}>
			Connect
		</button>
	</div>
	{#if errorMessage}
		<div class="alert-message error">
			<i class="fa-solid fa-triangle-exclamation"></i>
			{errorMessage}
		</div>
	{/if}
</label>

{#if checking === true}
	<p>Checking...</p>
{:else if checking === false}
	{#if errorMessage !== null}
		<aside class="alert variant-filled-warning mt-5">
			<i class="fa-solid fa-triangle-exclamation text-4xl"></i>
			<div class="alert-message" data-toc-ignore>
				<h3 class="h3" data-toc-ignore>Warning</h3>
				<p>Server is offline or do not exist</p>
			</div>
			<div class="alert-actions">
				<!-- <button class="btn variant-filled" on:click={triggerAction}>Action</button> -->
				<button class="btn-icon variant-filled" on:click={() => (checking = null)}>
					<i class="fa-solid fa-xmark"></i>
				</button>
			</div>
		</aside>
	{/if}
{/if}

<style>
	.error {
		height: fit-content;
		padding: 1;
		color: rgb(var(--color-error-400));
	}
	.limited {
		max-width: 400px;
	}
</style>
