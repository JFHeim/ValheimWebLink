import WebSocket from 'isomorphic-ws';

export const load = async () => {
	return {
		createWebSocket: () => {
			const ip = '192.168.128.1';
			const socket = new WebSocket(`ws://${ip}:8081/playerEye`);

			socket.onopen = () => {
				console.log('Connected to WebSocket server');
				socket.send(
					JSON.stringify({
						plName: 'Без Башки',
						message: 'Hello computer!'
					})
				);
			};

			socket.onclose = () => {
				console.log('Disconnected from WebSocket server');
			};

			socket.onerror = (error) => {
				console.error('WebSocket error:', error);
			};

			return socket;
		}
	};
};
