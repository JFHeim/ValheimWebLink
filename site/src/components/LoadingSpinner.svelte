<script>
	export let height = 'auto';
	export let width = 'auto';
</script>

<div style:width style:height>
	<div class="conic-gradient" data-testid="conic-gradient">
		<div
			class="block aspect-square rounded-full animate-spin"
			style="background: conic-gradient(transparent 0% 25%, rgb(var(--color-secondary-500)) 75% 100%);"
		></div>
	</div>
</div>
