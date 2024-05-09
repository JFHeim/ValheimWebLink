import os
import re

print("Hello, World!")
print("Hello, World!")
print("Hello, World!")
print("Hello, World!")
print("Hello, World!")
print("Hello, World!")


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


def find_controllers(directory):
    controllers = []
    for root, dirs, files in os.walk(directory):
        for file in files:
            if file.endswith(".cs"):  # Предполагаем, что контроллеры имеют расширение .cs
                file_path = os.path.join(root, file)
                with open(file_path, 'r', encoding='utf-8') as f:
                    content = f.read()
                    if '[Controller]' in content:
                        controllers.append(file_path)
    return controllers


def extract_info(file_path):
    info = {}
    with open(file_path, 'r', encoding='utf-8') as f:
        lines = f.readlines()
        for line in lines:
            if "public string Route" in line:
                info["Route"] = line.split('=>')[1].strip().strip('"')
            elif "public string HttpMethod" in line:
                info["HttpMethod"] = line.split('=>')[1].strip().strip('"')
            elif "public string Description" in line:
                info["Description"] = line.split('=>')[1].strip().strip('"')
    return info


def generate_readme_entries(controllers):
    readme_entries = []
    for controller in controllers:
        info = extract_info(controller)
        filename = os.path.basename(controller)
        if all(key in info for key in ["Route", "HttpMethod", "Description"]):
            entry = f"### {filename}\n{info['HttpMethod']} {info['Route']} - {info['Description']}"
            readme_entries.append(entry)
    return readme_entries


def update_readme(readme_path, readme_entries):
    with open(readme_path, 'w', encoding='utf-8') as f:
        for entry in readme_entries:
            f.write(entry + '\n')


if __name__ == "__main__":
    # Путь к директории с контроллерами
    directory = os.getcwd() + "/mod/Web/Controllers"
    readme_path = "README.md"  # Путь к README файлу

    print('Looking for new controllers in directory', directory)

    controllers = find_controllers(directory)
    readme_entries = generate_readme_entries(controllers)
    update_readme(readme_path, readme_entries)


# if __name__ == "__main__":
#     directory = os.getcwd()
#     print('Looking for new todos in directory', directory)
#     new_todo_pattern = re.compile(r"//TODO:\s*([^\n]+)")
#     old_todo_pattern = re.compile(r'#+\s*TODOs(.*?)(?=\n?#+|$)', re.DOTALL)
#     readme_path = os.path.join(directory, 'README.md')
#     overwrite_todos(find_new_todos(['.cs']))
