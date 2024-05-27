export const load = async () => {
	const checkLogin = async ({ login, password }) => {
		return {
			error: null,
			loginData: {
				username: login,
				role: 'full-admin'
			}
		};
	};

	return {
		checkLogin: checkLogin,
		title: 'Login'
	};
};
