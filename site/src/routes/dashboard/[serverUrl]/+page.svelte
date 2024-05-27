<script>
	import { counter } from '$lib/counter.js';

	export let data;
	/**@type {boolean | null}*/
	let checking = null;
	/** @type {{} | null}}*/
	let loginData = null;
	let loginErrorMsg = null;
	let passwordErrorMsg = null;

	const startLogin = async () => {
		let formData = {
			// @ts-ignore
			login: document.getElementsByName('login')[0].value,
			// @ts-ignore
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

		console.log(formData);

		checking = true;
		const result = await data.checkLogin(formData);
		loginData = result.loginData;
		console.log({ loginData });

		if (loginData) {
			checking = false;
			counter.set_loginData(loginData);
			counter.set_railId(1);
			console.log('Successfully logged in');
		}
	};
</script>

{#if checking === true}
	<p>Checking...</p>
{:else if checking === false}
	{#if !loginData}
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
{:else if checking === null}
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
						on:keyup={() => (loginErrorMsg = null)}
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
						on:keyup={() => (passwordErrorMsg = null)}
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
	.limited-login {
		@media (max-width: 1124px) {
			max-width: 450px;
			min-width: 450px;
		}

		@media (max-width: 930px) {
			max-width: 350px;
			min-width: 350px;
		}

		@media (max-width: 720px) {
			max-width: 300px;
			min-width: 300px;
		}

		@media (max-width: 470px) {
			max-width: 200px;
			min-width: 200px;
		}

		@media (min-width: 1500px) {
			max-width: 750px;
			min-width: 750px;
		}

		@media (min-width: 1300px) {
			max-width: 650px;
			min-width: 650px;
		}

		@media (min-width: 1123px) {
			max-width: 550px;
			min-width: 550px;
		}
	}
</style>
