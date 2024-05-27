<script>
	export let form;
</script>

<main class="flex items-center justify-center w-full h-screen bg-neutral-100">
	<form class="shadow-md rounded-sm w-[400px] p-8" method="POST">
		<h2 class="mb-8 text-2xl text-center text-neutral-950">Sign In</h2>
		{#if form}
			<h4 class="font-light text-center text-red-600 text-md">
				{form.error}
			</h4>
		{/if}
		<div class="mb-2">
			<label for="email" class="block ml-2 font-light text-neutral-800">email</label>
			<input
				type="email"
				id="email"
				name="email"
				placeholder="email"
				class="w-full p-2 border rounded-lg border-neutral-800"
			/>
		</div>
		<div class="mb-6">
			<label for="password" class="block ml-2 font-light text-neutral-800">password</label>
			<input
				class="w-full p-2 border rounded-lg border-neutral-800"
				type="password"
				id="password"
				name="password"
				placeholder="password"
			/>
		</div>
		<div class="flex justify-center w-full">
			<button
				type="submit"
				class="px-4 py-2 text-lg text-white bg-orange-600 rounded-full hover:bg-orange-500"
				>Submit</button
			>
		</div>
		<div class="flex justify-center w-full mt-8">
			<a href="/sign-up">Create Account</a>
		</div>
	</form>
</main>
