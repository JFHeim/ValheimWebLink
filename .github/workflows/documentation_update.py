import os
import re


class bcolors:
    HEADER = '\033[95m'
    OKBLUE = '\033[94m'
    OKCYAN = '\033[96m'
    OKGREEN = '\033[92m'
    WARNING = '\033[93m'
    FAIL = '\033[91m'
    ENDC = '\033[0m'
    BOLD = '\033[1m'
    UNDERLINE = '\033[4m'


def find_controllers():
    for root, dirs, files in os.walk(directory):
        for file in files:
            if not file.endswith(".cs"): continue
            file_path = os.path.join(root, file)
            with open(file_path, 'r', encoding='utf-8') as f:
                content = f.read()
                if '[Controller]' in content:
                    controllers.append(file_path)


def extract_info(file_path):
    info = {}
    with open(file_path, 'r', encoding='utf-8') as f:
        content = f.read()

        info["Route"] = re.search(r'public string Route\s*=>\s*"([^"]+)"', content).group(1)
        info["HttpMethod"] = re.search(r'public string HttpMethod\s*=>\s*"([^"]+)"', content).group(1)
        info["Description"] = re.search(r'public string Description\s*=>\s*"([^"]+)"', content).group(1)

        # Извлечение QueryParameters
        query_parameters_match = re.search(r'public List<QueryParamInfo>\s*QueryParameters\s*=>\s*\[([^]]*)]',
                                           content, re.DOTALL)
        if query_parameters_match:
            query_parameters_content = query_parameters_match.group(1)
            if query_parameters_content.strip() == "":  # Проверяем, пуст ли список QueryParameters
                info["QueryParameters"] = []
            else:
                query_parameters = []
                query_parameter_matches = re.finditer(r'new\(\s*"([^"]+)"\s*,\s*"([^"]+)"\s*,\s*"([^"]+)"\s*\)',
                                                      query_parameters_content)
                for match in query_parameter_matches:
                    query_parameters.append(
                        {"name": match.group(1), "type": match.group(2), "description": match.group(3)})
                info["QueryParameters"] = query_parameters
        else:
            info["QueryParameters"] = []
    return info


def generate_readme_entries():
    for controller in controllers:
        info = extract_info(controller)
        # filename = os.path.basename(controller)
        if all(key in info for key in ["Route", "HttpMethod", "Description"]):
            entry = (
                f'### {info['Route']}\n'
                f'Description: {info['Description']}<br>\n'
                f'HttpMethod: {info['HttpMethod']}<br>\n')

            query_parameters = info["QueryParameters"]
            if query_parameters:
                entry += 'Query parameters\n'
                for query_parameter in query_parameters:
                    param_name = query_parameter["name"]
                    param_type = query_parameter["type"]
                    param_description = query_parameter["description"]
                    entry += f'* {param_name}<br>\n'
                    entry += f'    Description: {param_description}<br>\n'
                    entry += f'    Type: {param_type}<br>\n'
                    entry += f'\n'
            else:
                entry += 'Has no query parameters\n'

            readme_entries.append(entry)


def update_readme():
    with open(readme_path, 'w', encoding='utf-8') as f:
        f.write(f'# Documentation\n')
        f.write(f'## Controllers\n')
        for entry in readme_entries:
            f.write(entry + '\n')


directory = os.getcwd() + "/mod/core/Web/Controllers"
readme_path = "DOCUMENTATION.md"
controllers = []
readme_entries = []
if __name__ == "__main__":
    print('Looking for new controllers in directory', directory)

    # clean readme file
    with open(readme_path, 'w', encoding='utf-8') as f:
        f.write("# Documentation\n## Controllers\n")
    print('Cleaned readme file -', readme_path, f'Content: {open(readme_path, "r", encoding="utf-8").read()}')

    find_controllers()
    generate_readme_entries()
    update_readme()
