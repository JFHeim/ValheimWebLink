<script>
	export let data;
	const { serverInfo } = data;
</script>

<a href="/dashboard">Dashboard</a>

<h1>Server information</h1>
{#await serverInfo}
	<p>Loading...</p>
{:then serverInfo}
	<pre>{JSON.stringify(serverInfo, null, 2)}</pre>
{/await}
