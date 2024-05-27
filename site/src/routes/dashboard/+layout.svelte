<script>
	import { browser } from '$app/environment';
	import { goto } from '$app/navigation';
	import { counter } from '$lib/counter';

	counter.subscribe((data) => {
		const { railId, serverUrl, loginData } = data;

		console.log('railId', railId, 'serverUrl', serverUrl);

		if (railId != null)
			switch (railId) {
				case 0:
					goto('/dashboard');
					break;
				case 1:
					goto(`/dashboard/${serverUrl}/${loginData.username}`);
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
