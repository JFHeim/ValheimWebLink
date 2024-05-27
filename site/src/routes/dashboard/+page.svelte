<script>
	import { counter } from '$lib/counter.js';
	import { hash } from '$lib/utils.js';
	import { onMount } from 'svelte';
	export let data;

	/**@type {boolean | null}*/
	let checking = null;
	/**@type {boolean | null}*/
	let isOnline = null;
	let loginData = null;
	/**@type {string | null}*/
	let inputErrorMsg = null;
	let loginErrorMsg = null;
	let passwordErrorMsg = null;
	/**@type {boolean}*/
	let showLogin = false;

	const init = () => {
		counter.set_railId(0);
	};

	counter.subscribe((value) => {});

	onMount(() => {
		init();

		const ipInput = document.getElementsByName('ip')[0];
		const portInput = document.getElementsByName('port')[0];

		if (counter.get().serverUrl != null && counter.get().serverUrl != 'null') {
			ipInput.setAttribute('value', counter.get().serverUrl.split(':')[0]);
			portInput.setAttribute('value', counter.get().serverUrl.split(':')[1]);
		} else if (localStorage.getItem('serverUrl')) {
			counter.set_serverUrl(localStorage.getItem('serverUrl'));
			ipInput.setAttribute('value', counter.get().serverUrl.split(':')[0]);
			portInput.setAttribute('value', counter.get().serverUrl.split(':')[1]);
			console.log(counter.get().serverUrl);
		}
	});

	const startServerCheck = async () => {
		showLogin = false;
		isOnline = null;
		checking = null;
		inputErrorMsg = null;
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
			inputErrorMsg = 'ip is required';
			return;
		}
		if (!validateIP()) {
			inputErrorMsg = 'ip is not valid';
			return;
		}
		if (!formData.port) {
			inputErrorMsg = 'port is required';
			return;
		}
		if (!validatePort()) {
			inputErrorMsg = 'port is not valid';
			return;
		}
		inputErrorMsg = null;

		console.log(formData);
		checking = true;
		const result = await data.isServerOnline(formData);
		isOnline = result.isOnline;
		console.log({ isOnline });
		checking = false;

		if (inputErrorMsg === null && isOnline === true) {
			showLogin = true;
			counter.set_serverUrl(`${formData.ip}:${formData.port}`);
		}
	};

	const startLogin = async () => {
		let formData = {
			login: document.getElementsByName('login')[0].value,
			password: document.getElementsByName('password')[0].value
		};
		console.log(formData);

		loginErrorMsg = null;
		passwordErrorMsg = null;

		if (!formData.login) {
			loginErrorMsg = 'ip is required';
			return;
		}
		if (!formData.password) {
			passwordErrorMsg = 'port is required';
			return;
		}

		let loginHash = await hash(formData.login);
		let passwordHash = await hash(formData.password);

		if (loginHash.error || passwordHash.error) {
			console.error('Hash error', loginHash.error, passwordHash.error);
			return;
		}

		formData = {
			login: loginHash.result,
			password: passwordHash.result
		};

		console.log(formData);

		// checking = true;
		// const result = await data.checkLogin(formData);
		// loginData = result.loginData;
		// console.log({ loginData });

		// if (loginData) {
		// 	showLogin = false;
		// 	counter.set_loginData(loginData);
		// 	counter.set_railId(1);
		// 	console.log('Successfully logged in');
		// }
	};
</script>

{#if showLogin === false}
	<label class="mt-4 label centered-rl">
		<h4 class="mb-2 h4 w-fit centered-rl">Search for a server</h4>
		<div class="flex gap-2 search-flex w-fit centered-rl">
			<div class="flex flex-row gap-2 w-fit">
				<div class="flex flex-col">
					<p class="centered">Ip address</p>
					<div class="flex centered-rl input-group input-group-divider limited-search">
						<input
							name="ip"
							class="input-number"
							placeholder="ip"
							type="text"
							on:keyup={() => (inputErrorMsg = null)}
						/>
					</div>
				</div>
				<div class="flex flex-col">
					<p class="centered">Port</p>
					<div class="flex centered-rl input-group input-group-divider limited-search">
						<input
							name="port"
							class="input-number"
							placeholder="port"
							on:keyup={() => (inputErrorMsg = null)}
						/>
					</div>
				</div>
			</div>
			<div>
				<button class="pl-3 pr-3 mt-6 btn variant-filled-secondary" on:click={startServerCheck}>
					<i class="fa-solid fa-magnifying-glass"></i>
					<span> Search</span>
				</button>
			</div>
		</div>
		{#if inputErrorMsg}
			<div class="alert-message error">
				<i class="fa-solid fa-triangle-exclamation"></i>
				{inputErrorMsg}
			</div>
		{/if}
	</label>
{/if}

{#if checking === true}
	<p>Checking...</p>
{:else if checking === false}
	{#if !isOnline}
		<aside class="mt-5 alert variant-filled-warning">
			<i class="text-4xl fa-solid fa-triangle-exclamation"></i>
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

{#if showLogin === true}
	<br />
	<div class="pb-1" />
	<h4 class="mb-2 h4 w-fit centered-rl">Login</h4>
	<div class="flex flex-col gap-2 centered-rl w-fit">
		<div>
			<div class="flex flex-col">
				<p class="centered"><i class="fa fa-user-circle" aria-hidden="true"></i> Username</p>
				<div class="flex centered-rl input-group input-group-divider limited-login">
					<input
						name="login"
						class=""
						placeholder="my cool name"
						type="text"
						on:keyup={() => (inputErrorMsg = null)}
					/>
				</div>
			</div>
			<div class="flex flex-col">
				<p class="centered"><i class="fa fa-key" aria-hidden="true"></i> Password</p>
				<div class="flex centered-rl input-group input-group-divider limited-login">
					<input
						name="password"
						class=""
						placeholder="12345623hashdkDTYTY$%&*%$"
						on:keyup={() => (inputErrorMsg = null)}
					/>
				</div>
			</div>
		</div>
		<div class="flex flex-row justify-center">
			<button class="pl-3 pr-3 mt-6 btn variant-filled-secondary" on:click={startLogin}>
				<i class="fa fa-link" aria-hidden="true"></i>
				<span> Connect</span>
			</button>
		</div>
	</div>
{/if}

<style>
	.error {
		height: fit-content;
		padding: 1;
		color: rgb(var(--color-error-400));
	}

	.limited-search {
		@media (max-width: 1124px) {
			max-width: 400px;
			min-width: 310px;
		}

		@media (max-width: 930px) {
			max-width: 250px;
			min-width: 250px;
		}

		@media (max-width: 720px) {
			max-width: 170px;
			min-width: 170px;
		}

		@media (max-width: 470px) {
			max-width: 130px;
			min-width: 130px;
		}

		@media (min-width: 1500px) {
			max-width: 700px;
			min-width: 700px;
		}

		@media (min-width: 1300px) {
			max-width: 500px;
			min-width: 500px;
		}

		@media (min-width: 1123px) {
			max-width: 400px;
			min-width: 310px;
		}
	}

	.search-flex {
		flex-direction: row;
		@media (max-width: 657px) {
			flex-direction: column;
		}
	}
	.limited-login {
		@media (max-width: 1124px) {
			max-width: 400px;
			min-width: 310px;
		}

		@media (max-width: 930px) {
			max-width: 250px;
			min-width: 250px;
		}

		@media (max-width: 720px) {
			max-width: 170px;
			min-width: 170px;
		}

		@media (max-width: 470px) {
			max-width: 130px;
			min-width: 130px;
		}

		@media (min-width: 1500px) {
			max-width: 700px;
			min-width: 700px;
		}

		@media (min-width: 1300px) {
			max-width: 500px;
			min-width: 500px;
		}

		@media (min-width: 1123px) {
			max-width: 400px;
			min-width: 310px;
		}
	}
</style>
