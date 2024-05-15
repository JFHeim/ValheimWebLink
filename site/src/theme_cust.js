export const theme_cust = {
	name: 'theme_cust',
	properties: {
		// =~= Theme Properties =~=
		'--theme-font-family-base': `system-ui`,
		'--theme-font-family-heading': `Inter, ui-sans-serif, system-ui, -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, 'Noto Sans', sans-serif, 'Apple Color Emoji', 'Segoe UI Emoji', 'Segoe UI Symbol', 'Noto Color Emoji'`,
		'--theme-font-color-base': 'var(--color-surface-700)',
		'--theme-font-color-dark': 'var(--color-primary-200)',
		'--theme-rounded-base': '8px',
		'--theme-rounded-container': '12px',
		'--theme-border-base': '2px',
		// =~= Theme On-X Colors =~=
		'--on-primary': 'var(--color-surface-800)',
		'--on-secondary': 'var(--color-surface-800)',
		'--on-tertiary': 'var(--color-primary-200)',
		'--on-success': '0 0 0',
		'--on-warning': '0 0 0',
		'--on-error': '255 255 255',
		'--on-surface': 'var(--color-primary-200)',
		// =~= Theme Colors  =~=
		// primary | #d4b468
		'--color-primary-50': '249 244 232', // #f9f4e8
		'--color-primary-100': '246 240 225', // #f6f0e1
		'--color-primary-200': '244 236 217', // #f4ecd9
		'--color-primary-300': '238 225 195', // #eee1c3
		'--color-primary-400': '225 203 149', // #e1cb95
		'--color-primary-500': '212 180 104', // #d4b468
		'--color-primary-600': '191 162 94', // #bfa25e
		'--color-primary-700': '159 135 78', // #9f874e
		'--color-primary-800': '127 108 62', // #7f6c3e
		'--color-primary-900': '104 88 51', // #685833
		// secondary | #889c6d
		'--color-secondary-50': '237 240 233', // #edf0e9
		'--color-secondary-100': '231 235 226', // #e7ebe2
		'--color-secondary-200': '225 230 219', // #e1e6db
		'--color-secondary-300': '207 215 197', // #cfd7c5
		'--color-secondary-400': '172 186 153', // #acba99
		'--color-secondary-500': '136 156 109', // #889c6d
		'--color-secondary-600': '122 140 98', // #7a8c62
		'--color-secondary-700': '102 117 82', // #667552
		'--color-secondary-800': '82 94 65', // #525e41
		'--color-secondary-900': '67 76 53', // #434c35
		// tertiary | #525c47
		'--color-tertiary-50': '229 231 227', // #e5e7e3
		'--color-tertiary-100': '220 222 218', // #dcdeda
		'--color-tertiary-200': '212 214 209', // #d4d6d1
		'--color-tertiary-300': '186 190 181', // #babeb5
		'--color-tertiary-400': '134 141 126', // #868d7e
		'--color-tertiary-500': '82 92 71', // #525c47
		'--color-tertiary-600': '74 83 64', // #4a5340
		'--color-tertiary-700': '62 69 53', // #3e4535
		'--color-tertiary-800': '49 55 43', // #31372b
		'--color-tertiary-900': '40 45 35', // #282d23
		// success | #afff38
		'--color-success-50': '243 255 225', // #f3ffe1
		'--color-success-100': '239 255 215', // #efffd7
		'--color-success-200': '235 255 205', // #ebffcd
		'--color-success-300': '223 255 175', // #dfffaf
		'--color-success-400': '199 255 116', // #c7ff74
		'--color-success-500': '175 255 56', // #afff38
		'--color-success-600': '158 230 50', // #9ee632
		'--color-success-700': '131 191 42', // #83bf2a
		'--color-success-800': '105 153 34', // #699922
		'--color-success-900': '86 125 27', // #567d1b
		// warning | #d2cb0f
		'--color-warning-50': '248 247 219', // #f8f7db
		'--color-warning-100': '246 245 207', // #f6f5cf
		'--color-warning-200': '244 242 195', // #f4f2c3
		'--color-warning-300': '237 234 159', // #edea9f
		'--color-warning-400': '224 219 87', // #e0db57
		'--color-warning-500': '210 203 15', // #d2cb0f
		'--color-warning-600': '189 183 14', // #bdb70e
		'--color-warning-700': '158 152 11', // #9e980b
		'--color-warning-800': '126 122 9', // #7e7a09
		'--color-warning-900': '103 99 7', // #676307
		// error | #c90808
		'--color-error-50': '247 218 218', // #f7dada
		'--color-error-100': '244 206 206', // #f4cece
		'--color-error-200': '242 193 193', // #f2c1c1
		'--color-error-300': '233 156 156', // #e99c9c
		'--color-error-400': '217 82 82', // #d95252
		'--color-error-500': '201 8 8', // #c90808
		'--color-error-600': '181 7 7', // #b50707
		'--color-error-700': '151 6 6', // #970606
		'--color-error-800': '121 5 5', // #790505
		'--color-error-900': '98 4 4', // #620404
		// surface | #2b4036
		'--color-surface-50': '223 226 225', // #dfe2e1
		'--color-surface-100': '213 217 215', // #d5d9d7
		'--color-surface-200': '202 207 205', // #cacfcd
		'--color-surface-300': '170 179 175', // #aab3af
		'--color-surface-400': '107 121 114', // #6b7972
		'--color-surface-500': '43 64 54', // #2b4036
		'--color-surface-600': '39 58 49', // #273a31
		'--color-surface-700': '32 48 41', // #203029
		'--color-surface-800': '26 38 32', // #1a2620
		'--color-surface-900': '21 31 26' // #151f1a
	}
};
