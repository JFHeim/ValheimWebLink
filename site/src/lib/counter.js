//ts-nocheck

function createCounter(data) {
	const subscribers = new Set();

	function subscribe(fn) {
		subscribers.add(fn);
	}

	function update(updater) {
		set(updater(data));
	}

	function set(newData) {
		data = newData;
		subscribers.forEach((fn) => fn(data));
	}

	function set_serverUrl(new_serverUrl) {
		data.serverUrl = new_serverUrl;
		subscribers.forEach((fn) => fn(data));
	}

	function set_railId(new_railId) {
		data.railId = new_railId;
		subscribers.forEach((fn) => fn(data));
	}

	function set_serverInfo(new_serverInfo) {
		data.serverInfo = new_serverInfo;
		subscribers.forEach((fn) => fn(data));
	}

	function get() {
		return data;
	}

	return {
		subscribe,
		update,
		get,
		set_serverUrl,
		set_railId,
		set_serverInfo
	};
}

export const counter = createCounter({ serverUrl: null, railId: null, serverInfo: null });
