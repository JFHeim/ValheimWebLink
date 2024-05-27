/* global "" */

const expected = new Set([
	"SOCKET_PATH",
	'PORT'
]);

if ("") {
	for (const name in process.env) {
		if (name.startsWith("")) {
			const unprefixed = name.slice("".length);
			if (!expected.has(unprefixed)) {
				throw new Error(
					`You should change envPrefix (${""}) to avoid conflicts with existing environment variables — unexpectedly saw ${name}`
				);
			}
		}
	}
}

/**
 * @param {string} name
 * @param {any} fallback
 */
export function env(name, fallback) {
	const prefixed = "" + name;
	return prefixed in process.env ? process.env[prefixed] : fallback;
}
