<script>
	import { browser } from '$app/environment';
	import { goto } from '$app/navigation';
	import { counter } from '$lib/counter';

	export const ssr = false;

	counter.subscribe((data) => {
		const { railId, serverUrl } = data;
		if (railId != null)
			switch (railId) {
				case 0:
					goto('/dashboard');
					break;
				case 1:
					goto(`/dashboard/${serverUrl}/`);
					break;
				// 	case 2:
				// 		goto(`/dashboard/${serverUrl}/worlds/`);
				// 		break;
				// 	default:
				// 		console.error('Unknown rail id', railId);
				// 		goto('/dashboard');
				// 		break;
			}
	});
</script>

<slot />
